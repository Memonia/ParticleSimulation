using System.Runtime.InteropServices;

using ParticleSimulation.Core.Collisions;
using ParticleSimulation.Core.Extensions;
using ParticleSimulation.Core.Interface;
using ParticleSimulation.Core.Native.StructureWrappers;

namespace ParticleSimulation.Core.Native
{
    internal unsafe class NativeSimulator : ISimulator
    {
        /* All relevant comments are in 'Simulator' */

        private const CallingConvention CC = CallingConvention.Cdecl;
        private const string _name = "ParticleSimulation.Core.CUDA.dll";

        [DllImport(_name, CallingConvention = CC)]
        private extern static IntPtr getSimulatorHandle(
            NativeCollidables collidables,
            NativeSimulatorInfo simulatorInfoOut,
            NativeSimulationStateOut simulationStateOut
        );

        [DllImport(_name, CallingConvention = CC)]
        private extern static bool nextSimulationState(IntPtr simulatorHandle);

        [DllImport(_name, CallingConvention = CC)]
        private extern static void startSimulation(IntPtr simulatorHandle);

        [DllImport(_name, CallingConvention = CC)]
        private extern static void stepSimulation(IntPtr simulatorHandle);

        [DllImport(_name, CallingConvention = CC)]
        private extern static void stopSimulation(IntPtr simulatorHandle);

        [DllImport(_name, CallingConvention = CC)]
        private extern static void joinSimulator(IntPtr simulatorHandle);

        [DllImport(_name, CallingConvention = CC)]
        private extern static void freeHandle(IntPtr simulatorHandle);

        private bool _wasReset = true;
        private bool _isRunning = false;
        private bool _forceStop = false;
        private bool disposedValue = false;

        private readonly Container _container;

        private SimulationState _currentCommitedState;

        private IntPtr _simulatorHandle;

        private NativeSimulatorInfo _simulatorInfo;

        private NativeCollidables _nativeCollidables;

        private NativeSimulationStateOut _currentNativeCommitedState;

        public int QueueLength => *(int*)_simulatorInfo.QueueLength;

        public FrameInfo CurrentFrameInfo => new ( _currentCommitedState.Collisions);

		public IReadOnlyCollection<IParticle> Particles => _currentCommitedState.Particles;

		public NativeSimulator(Container container)
        {
            _container = container;
        }

        private void _freeNativeResources()
        {
            freeHandle(_simulatorHandle);
            Marshal.FreeHGlobal(_nativeCollidables.Surfaces);
            Marshal.FreeHGlobal(_nativeCollidables.Particles);
            Marshal.FreeHGlobal(_simulatorInfo.QueueLength);
            Marshal.FreeHGlobal(_currentNativeCommitedState.Collisions);
        }

        private void _initNativeCollidables(Collidables collidables)
        {
            var nativeSurfaces = collidables.Surfaces
                .Select(e => e.ToNative()).ToArray();

            var nativeParticles = collidables.Particles
                .Select(e => e.ToNative()).ToArray();

            var s_size = Marshal.SizeOf<NativeSurface>();
            var p_size = Marshal.SizeOf<NativeParticle>();

            IntPtr s_ptr = Marshal.AllocHGlobal(s_size * collidables.Surfaces.Count);
            IntPtr p_ptr = Marshal.AllocHGlobal(p_size * collidables.Particles.Count);

            IntPtr ptr = s_ptr;
            for (int i = 0; i < collidables.Surfaces.Count; ++i, ptr += s_size)
			{
				Marshal.StructureToPtr(nativeSurfaces[i], ptr, false);
			}

			ptr = p_ptr;
            for (int i = 0; i < collidables.Particles.Count; ++i, ptr += p_size)
			{
				Marshal.StructureToPtr(nativeParticles[i], ptr, false);
			}

			_nativeCollidables = new NativeCollidables()
            {
                Surfaces = s_ptr,
                SurfaceCount = nativeSurfaces.Length,
                Particles = p_ptr,
                ParticleCount = nativeParticles.Length
            };

            _simulatorInfo.QueueLength =
                Marshal.AllocHGlobal(Marshal.SizeOf<int>());
            Marshal.WriteInt32(_simulatorInfo.QueueLength, 0);

            _currentNativeCommitedState.Particles = p_ptr;
            _currentNativeCommitedState.Collisions =
                Marshal.AllocHGlobal(Marshal.SizeOf<int>());
            Marshal.WriteInt32(_currentNativeCommitedState.Collisions, 0);
        }

        public void Step()
        {
            if (_isRunning)
			{
				throw new InvalidOperationException("Simulation is still running");
			}

			stepSimulation(_simulatorHandle);
        }

        public void StartSimulation(CancellationToken token)
        {
            if (_isRunning)
			{
				throw new InvalidOperationException(
                    "Simulation has been already started"
                );
			}

			token.Register(() =>
            {

                stopSimulation(_simulatorHandle);
                _isRunning = false;
            });

            startSimulation(_simulatorHandle);

            _wasReset = false;
            _isRunning = true;
        }

        public bool Next()
        {
            if (nextSimulationState(_simulatorHandle))
            {
				_currentCommitedState.Collisions = *(int*)_currentNativeCommitedState.Collisions;

                for (int i = 0; i < _nativeCollidables.ParticleCount; ++i)
                {
                    _currentCommitedState.Particles[i].Center =
                        _nativeCollidables.ParticlesPtr[i].Center.FromNative();

                    _currentCommitedState.Particles[i].Velocity =
                        _nativeCollidables.ParticlesPtr[i].Velocity.FromNative();
                }

                return true;
            }

            return false;
        }

        public void Reset()
        {
            if (_isRunning)
			{
				throw new InvalidOperationException("Simulation is still running");
			}

			_container.Clear();
            _freeNativeResources();

            _wasReset = true;
        }

        public void Join()
        {
            joinSimulator(_simulatorHandle);
        }

        public void SpawnParticles(SpawnParameters spawnParams, int? seed = null)
        {
            if (_isRunning)
			{
				throw new InvalidOperationException("Simulation is still running");
			}

			if (!_wasReset)
			{
				throw new InvalidOperationException("Simulation was not reset");
			}

			_container.Spawn(spawnParams, seed);
            _currentCommitedState = new() 
            { 
                Collisions = 0,
                Particles = _container.Collidables.Particles.DeepCopy()
            };

			_initNativeCollidables(_container.Collidables);
            _simulatorHandle = getSimulatorHandle(
                _nativeCollidables,
                _simulatorInfo,
                _currentNativeCommitedState
            );

            _wasReset = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                stopSimulation(_simulatorHandle);
                joinSimulator(_simulatorHandle);

				if (!_wasReset)
				{
					_freeNativeResources();
				}

				disposedValue = true;
            }
        }

        ~NativeSimulator()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

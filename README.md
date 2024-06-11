# ParticleSimulation
A visualisator of particle interactions, based on an ideal gas model ([see wikipedia](https://en.wikipedia.org/wiki/Ideal_gas)). The simulation can be run on either a CPU or a GPU.

* 2021: First version 
* 2023: Second version. Written from scratch, added CUDA module

## GPU Acceleration Requirements
  * CUDA Toolkit 12.5
  * CUDA GPU Compute Capability >= 5.2
 
 CUDA Toolkit installer takes care of setting up Visual Studio. [See the installation guide.](https://docs.nvidia.com/cuda/cuda-installation-guide-microsoft-windows/)

## Possible problems
### ParticleSimulation.Core.Cuda failing to load
 CUDA project will fail to load if the installed CUDA Toolkit version doesn't match exactly the one specified in the .vcxproj file of the project. In this case open the mentioned file and look for the following items:
```
  <ImportGroup Label="ExtensionSettings">
    <Import Project="$(VCTargetsPath)\BuildCustomizations\CUDA 12.5.props"/>
  </ImportGroup>

  <ImportGroup Label="ExtensionTargets">
    <Import Project="$(VCTargetsPath)\BuildCustomizations\CUDA 12.5.targets"/>
  </ImportGroup>
```
Then change the version (in this case 12.5) to the version of CUDA Toolkit you have installed. 

Changing CUDA settings (do with caution, something might break):
 * CUDA Toolkit version can be changed in the 'build customization' of the loaded project
 * CUDA GPU Compute Capability to be compiled for can be changed in 'CUDA C/C++' configuration property of the project

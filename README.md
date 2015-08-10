# Space Sketchr

### Releasable build v1.0
####
#####


This code is able to run / export the latest version of Tango Doodle using Unity 4.



### Overview

As with any Tango project, the first step is to import the Tango assets into your project.

Download from here: https://developers.google.com/project-tango/downloads

Go to `Assets > Import Package > Custom package` and select the package file. Leave all assets selected and import them. You should now have a `TangoSDK` folder in your project.

(NOTE: At this point, unity might be reporting errors in the tango source code. If so, build the project once `File > Build` and they should go away)

As with all Unity / Tango apps, you'll need to add the prefab at `TangoSDK/Core/Prefabs/Tango Manager` to your scene, and take a look at it's components. We only need pose data for this demo, not depth, so leave everything as it is.

Take a look at the `DrawingRig` object in the root of the scene. All of the drawing logic happens in the `Sketchpad.cs` script, which is attached to a particle system. The "drawing" is actually a regular old ParticleSystem. The only difference is that Looping, Play on awake, Emission, and Shape are all disabled (Leaving Renderer on).

Note that `DrawingRig` has a `TangoSketchController` script on it. This is a copy of `TangoSDK/Examples/Scripts/Controllers/SampleController.cs`, with a bunch of `OnGUI` debug info disabled. This script is what takes the latest pose (position + rotation) data from the device and applies it to the game object.

The actual drawing logic, in `Sketchpad.cs`, is fairly standard unity code. Here's how it works:

- Listen for mouse events
- Project a ray onto the `InvisibleCanvas` plane that is attached to `DrawingRig`
- Create a bunch of new points from the last point to the new point
- Update the ParticleSystem with the latest points

Check out the source of `Sketchpad.cs` for more detail.

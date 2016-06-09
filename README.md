![Imgur](http://i.imgur.com/ZQqvwKf.png?1)

UnityNEAT
=========

UnityNEAT is a port of [SharpNEAT] from pure C# 4.0 to Unity 4.x and 5 (using Mono 2.6), and is integrated to work with Unity scenes for evaluation. UnityNEAT is created by Daniel Jallov as part of his [master's thesis] at the [Center for Computer Games Research] at the IT University in Copenhagen.

All the NEAT code is pure SharpNEAT, but is running in a single thread through Coroutines instead of using Parallel.For as in regular SharpNEAT.

Usage
-----

To use NEAT in your Unity project, simply download the UnityNEAT folder and import it to your Unity project. In the folder you will find a few example experiments that you can copy and use for inspiration.

You will need to follow these steps to set up your scene to use UnityNEAT:

1. In your scene, add an empty game object ("Evaluator") and attach the **Optimizer** class to it. In the Optimizer script drag your Unit prefab to the Unit slot. Your unit is the thing which you are evaluating; a car, a robot, Pac Man, a colourful 3D shape. In this script you can set the trial duration (how many seconds each unit is evaluated) and the number of trials, which is useful if you use randomness.
    
2. Attach a script to your Unit prefab which extends UnitController instead of MonoBehaviour. Remember to use **FixedUpdate** instead of Update if the Time is scaled.
```c#
public class CarController : UnitController {
    bool IsRunning;
    IBlackBox box;

    void FixedUpdate() {
        ..
    }

    public override void Stop()
    {
        this.IsRunning = false;
    }

    public override void Activate(IBlackBox box)
    {
        this.box = box;
        this.IsRunning = true;
    }
    
    public override float GetFitness()
    {
        // Implement a meaningful fitness function here, for each unit.
        return 0;
    }
}
```

3. In the folder Resources there is a file called ```experiment.config.xml``` in which you can specify the parameters for the evolution, such as ComplexityRegulationStrategy and PopulationSize. See the original SharpNEAT project for more settings.

4. In the ```Optimizer``` script you can set the number of inputs and outputs in the consts ```NUM_INPUTS``` and ```NUM_OUTPUTS```. The fields ```champFileSavePath``` and ```popFileSavePath``` are the locations where the xml file with the population and best individual is saved. By default it is saved to your local application path, which is in your User folder. The location will be printed in the console when run.

Example: Car experiment
------

In the Test scene folder you will find an example experiment with a simple car racing around a race track. In the folder CarExperiment you can find the CarController which calculates the fitness of the individual by measuring which road pieces the car goes through and how many laps it does. The car is penalized for hitting the walls.

To set up the Car experiment properly in a new project, you need to set up the physics layers in your project. To do so, go to Edit --> Project Settings --> Tags and layers and add a new layer and name it Car. Then go into Edit --> Project Settings --> Physics and remove the tick from the Layer Collision Matrix for Car-Car (should be the bottom most one). Then, go to CarExperiment folder and select the jeep prefab and set its layer to Car.

A video of the evolution can be seen on [youtube].

License
------

This project is released under the same license as SharpNEAT: http://sharpneat.sourceforge.net/licensing.htm
which is the [MIT License], changed from GNU General Public License, minimum version 3 in a previous version.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.



[SharpNEAT]:http://sharpneat.sourceforge.net/
[Center for Computer Games Research]:http://game.itu.dk/index.php/About
[master's thesis]:http://jallov.com/thesis
[youtube]:http://youtu.be/sHc9u67JPWc
[MIT License]:http://opensource.org/licenses/MIT

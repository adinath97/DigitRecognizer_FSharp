namespace FSharpLibrary

module Say =
//saying "open ..." in F# is equivalent to saying "using..." in C#
open System.IO;

// creating type X with variables as typed below is equivalent to in C# creating class X
// a type is a record (essentially an immutable class) in F#
// we can instantiate a record simply by adding braces and filling out the desired properties
// ex. instantiation : let obs = { Label = "x" ; Pixels = [|1;2;3;4|] }
// note the manner in which the array is initialized ^^

type Observation = { Label: string ; Pixels: int[] }

let toObservation (csvData:string) =
    let columns = csvData.Split(',')
    let label = columns.[0]
    let pixels = columns.[1..] |> Array.map int 
    { Label = label ; Pixels = pixels }
    //in the let statement in composing lines 19, we take the pixel value from column 0 till the last one, and add/map it to an array of integers
    //in line 20, we let the label = label defined in line 18, and let pixels = pixels defined in line 19. This way, our function returns an observation!

// Instead of defining a class with methods that create observations, we define a function that takes
// the path/file as input and uses another function (ReadAllLines) to convert every line into an observation
// note how we apply the toObservation function defined above to handle the actual conversion of each
// line into an observation, which is then added to an array of observations. The function reader essentially
// goes thru the lines in the CSV file one by one, applies the toObservation function, then adds the result
// to an array
let reader path =
    let data = File.ReadAllLines path //reads every line in the path file
    data.[1..] |> Array.map toObservation //[1..] => index from 1 till the final element in the data set created in the line above
    //line above maps the data to an associated observation


let trainingPath = @"/Users/adityaiyengar/Documents/C#/DigitRecognition/Classification-DigitRecognition/train.csv"
let trainingData = reader trainingPath //let trainingData be the set of observation obtained from trainingPath

//define a distance function (instead of, say, defining a distance class and interface)
//note: use 'fun' keyword to define map to the expression.
let manhattanDistance  (pixels1, pixels2) = 
    Array.zip pixels1 pixels2 //.zip: 2 arrays => array of tuples
    |> Array.map ( fun (x,y) -> abs(x-y)) //map each tuple to the absolute value of their difference
    |> Array.sum //adds the elements of the array of differences of absolute values, and thus gives the "difference" between two images

//define a function that makes our predictions
let train (trainingset:Observation[]) = //input is our training set and is in the form of an array of observations
    let classify (pixels:int[]) =  //input is our training set and is in the form of an array of observations
        trainingset
        //now below we map each element in the training set (ie. each observation) to the "distance" between
        //that element's pixels to the pixels passed in as an input. Then, after completing the function
        //, determine the minimum value (ie. minimum distance) in the array:
        |> Array.minBy (fun x -> manhattanDistance (x.Pixels,pixels))
        //now that we have the x/observation with the minumum distance, we seek to obtain it's label:
        |> fun x -> x.Label
    classify //train returns the function defined above

//when we pass a set into train, we let that set = trainingset , pass the trianing set into classify, the return the classify function for further use
//the input of classify is an array of type int (the "unknown"), while the return type is the label (which is of type string)
let classifier = train trainingData //let classifier contain the output of the train function with input trainingData
//classifer thus contains the classify function where trainingset = trainingData

//testing data
let testingPath = @"/Users/.../train_2.csv"
let testingData = reader testingPath //pass our testing path into reader, and reader will output an associated array of observations

let finalValue = Array.averageBy (fun x -> if classifier (x.Pixels) = x.Label then 1. else 0.) testingData
printfn "Model accuracy: %f" finalValue
 

Feature: Generate a sequence diagram
Business rules:
 - A sequence diagram can be generated for a method
 - The user specifies: 
	- which csharp project should be analyzed
	- which class should be analyzed
	- which method should be analyzed
 - The diagram is generated using the mermaidJS syntax
 - Method calls within the same type are not modeled
	- Method calls to other types made in referenced calls are modeled
 - Method calls to other classes are modeled
 - When multiple calls to other class(es) occur, the caller is 'activated' 

Scenario: Sequence diagram with two calls to another type
	When I call Livign.CodeToDesign with the following parameters
		| SolutionFile | Project      | Class               | Method                        |
		| Livign.sln   | TestProject1 | TestProject1.Actor1 | TwoDifferentCallsToOtherActor |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Actor1.TwoDifferentCallsToOtherActor' entry in the resx

Scenario: Sequence diagram with a call to another type via a method on the same type
	When I call Livign.CodeToDesign with the following parameters
		| SolutionFile | Project      | Class               | Method                              |
		| Livign.sln   | TestProject1 | TestProject1.Actor1 | OneCallToOtherActorViaPrivateMethod |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Actor1.OneCallToOtherActorViaPrivateMethod' entry in the resx

Scenario: Sequence diagram with a call to another type that assigns the return value
	When I call Livign.CodeToDesign with the following parameters
		| SolutionFile | Project      | Class               | Method                            |
		| Livign.sln   | TestProject1 | TestProject1.Actor1 | OneCallToOtherActorWithAssignment |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Actor1.OneCallToOtherActorWithAssignment' entry in the resx

Scenario: Sequence diagram with a static call to another type
	When I call Livign.CodeToDesign with the following parameters
		| SolutionFile | Project      | Class               | Method                    |
		| Livign.sln   | TestProject1 | TestProject1.Actor1 | OneStaticCallToOtherActor |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Actor1.OneStaticCallToOtherActor' entry in the resx

Scenario: Sequence diagram with a call to another type in an assembly without symbols
	When I call Livign.CodeToDesign with the following parameters
		| SolutionFile | Project      | Class               | Method                                    |
		| Livign.sln   | TestProject1 | TestProject1.Actor1 | OneCallToAClassFromAssemblyWithoutSymbols |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Actor1.OneCallToAClassFromAssemblyWithoutSymbols' entry in the resx

Scenario: Sequence diagram with a recursive call
	When I call Livign.CodeToDesign with the following parameters
		| SolutionFile | Project      | Class               | Method                     |
		| Livign.sln   | TestProject1 | TestProject1.Actor1 | SelfCallingRecursiveMethod |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Actor1.SelfCallingRecursiveMethod' entry in the resx

Scenario: Sequence diagram with a call that recurs in 3 steps
	When I call Livign.CodeToDesign with the following parameters
		| SolutionFile | Project      | Class               | Method                     |
		| Livign.sln   | TestProject1 | TestProject1.Actor1 | MethodThatRecursIn3Steps_1 |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Actor1.MethodThatRecursIn3Steps_1' entry in the resx

Scenario: Temp
	When I call Livign.CodeToDesign with the following parameters
		| SolutionFile | Project             | Class                                        | Method        |
		| Livign.sln   | Livign.CodeToDesign | Livign.CodeToDesign.SequenceDiagramGenerator | GenerateAsync |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Actor1.OneCallToOtherActorViaPrivateMethod' entry in the resx
#Test with a call on the result of a method
#Test with a call to a private methods
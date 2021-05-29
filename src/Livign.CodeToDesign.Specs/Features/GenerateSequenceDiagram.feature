﻿Feature: Generate a sequence diagram

Business rules:
 - A sequence diagram can be generated for a method
 - The user specifies: 
	- which csharp project should be analyzed
	- which class should be analyzed
	- which method should be analyzed
 - The diagram is generated using the mermaidJS syntax
 - Method calls to other classes are modeled
 - When multiple calls to other class(es) occur, the caller is 'activated' 

Scenario: Sequence diagram with a call to another class
	When I call Livign.CodeToDesign with the following parameters
		| Project      | Class | Method  |
		| TestProject1 | Car   | DriveTo |
	Then the result should be equal to the 'SequenceDiagram.TestProject1.Car.DriveTo' entry in the resx
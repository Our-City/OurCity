The API layer uses a Vertical Slice Architecture design. Use cases are per file, and have multiple things all relating to the endpoint from business logic for co-location. which does NOT break SRP since yea like it would change because the createpost use case as a whole is a unit ahahaha
- some of the reasons for switching to VSA
  - code that changes together lives together
  - screaming architecture -> folders/files tell reader about system
- some of the reasons for splitting into individula usecases/endpoints 
  - work on features in isolation (want to add a CreatePost usecase -> just add that)
  - Only ADDING code -> how could you break existing code?
  - see practically everything related to that endpoint in one file. far easier mental load

all in all we wanna do this and we thought about it and we think its better to work in so dont dock marks lololol
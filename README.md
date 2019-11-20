# Oppsyn
Oppsyn is a slack bot to help add metadata and information to the pictures in your slack channels. Running on dotnet core and built to run in a docker container.

## About
 - Identifies images uploaded or linked in channels of your slack team
 - Uses cognitive services to analyze the images
 - Posts identifying and descriptive metadata about the images

## How To Run
Build using `dotnet build` or by using the docker file to `docker build`. 
Map up your docker image with a volume or use input parameter to provide your config.json with slack tokens and azure access token.
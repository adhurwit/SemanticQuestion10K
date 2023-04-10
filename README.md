# SemanticQuestion10K

Ask the Microsoft 2022 10K a question and get an answer using Semantic Kernel and OpenAI Embeddings. 

!(10k-a-and-a.png)


This is a sample project that shows the basics of how to ask questions to a document 
- in this case Microsoft's 10K statement for 2022 - using the Semantic Kernel package. 

Embeddings are used to create a semantic database. When you ask a question, the database is searched for similar sentences. 
A prompt is crafted from these sentences and sent to an OpenAI GPT-3 model in Azure OpenAI Service to create an answer.


## Setup

1. An Azure OpenAI Service is required to run this project. 
You can sign up for a free trial here: https://azure.microsoft.com/en-us/services/openai-service/

2. A Qdrant vector database is used to store the embeddings. You can easily run the Qdrant database in a container 
and map a volume from your drive to the container.

3. There are two assembly references in this project that refer to the Semantic Kernel project. 
You will need to download the project from the Semantic Kernel repo, build it, and add the references to the project.

4. I have included a text file in the docs folder which is just the 10K document saved as text - you will need this to create 
the smemantic database. 

5. Provide the following variables through user secrets:


  dotnet user-secrets set "QDRANT_ENDPOINT" "http://localhost"
  dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "your-azure-openai-service-endpoint"
  dotnet user-secrets set "AZURE_OPENAI_KEY "your azure openai service key"



## Usage

There are two functions to run in the project: Parse and Question.

1. Parse: This will parse the 10K document and store the embeddings in the Qdrant database. 
It expects the location of the text file (provided in the docs folder).

`
SemanticQuestion10K.exe --parse --tenkfile c:\path to file\ms10k.txt 
`


2. Question: This will start a loop where you can ask questions and get answers from the content of the file.

`
SemanticQuestion10K.exe --question
`






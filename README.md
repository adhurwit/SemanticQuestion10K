# SemanticQuestion10K

Ask the Microsoft 2022 10K questions and get answers using Microsoft Semantic Kernel and Azure OpenAI Service. 

![image](https://raw.githubusercontent.com/adhurwit/SemanticQuestion10K/main/10k-q-and-a.png)


This is a sample project that shows the basics of how to ask questions to a document using the Semantic Kernel project (https://github.com/microsoft/semantic-kernel). For this sample I have used Microsoft's 10K statement for 2022. 

Embeddings are used to create a semantic database. When you ask a question, the database is searched for similar sentences. 
A prompt is crafted from these sentences and sent to an OpenAI GPT-3 model in Azure OpenAI Service to create an answer.


**NOTE: this is sample code for demonstration purposes only and is not intended for production use nor is it supported in any way.** 

## Setup

1. An Azure OpenAI Service is required to run this project. https://azure.microsoft.com/en-us/services/openai-service/

2. A Qdrant vector database (https://github.com/qdrant/qdrant) is used to store the embeddings. You can easily run the Qdrant database in a container and map a volume from your drive to the container. https://qdrant.tech/documentation/quick_start/

3. There are two assembly references in this project that refer to the Semantic Kernel project. https://github.com/microsoft/semantic-kernel  You will need to download the project from the Semantic Kernel repo, build it, and add the references to the project. 

4. I have included a text file in the docs folder which is just the 10K document saved as text - you will need this to create 
the smemantic database. 

5. Provide the following variables through user secrets:

`dotnet user-secrets set "QDRANT_ENDPOINT" "http://localhost"`

`dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "your-azure-openai-service-endpoint"`

`dotnet user-secrets set "AZURE_OPENAI_KEY "your azure openai service key"`



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

## TODO
Many things to improve, would love to hear feedback.. 




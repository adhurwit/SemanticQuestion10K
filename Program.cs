
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using RepoUtils;
using BlingFire;

namespace SemanticQuestion10K
{
    internal class Program
    {

        const string memoryCollectionName = "Microsoft10K";
        static void Main(string[] args)
        {
            bool parse = false;
            bool question = false;
            string tenkfile = "ms10k.txt";

            //loop through args with an integer
            for(int i=0; i<args.Length; i++)
            {
                if (args[i] == "--parse") parse = true;
                if (args[i] == "--question") question = true;
                if (args[i].StartsWith("--tenkfile")) tenkfile = args[i+1];
            }

            QdrantMemoryStore memoryStore = new QdrantMemoryStore(Env.Var("QDRANT_ENDPOINT"), 6333, vectorSize: 1536, ConsoleLogger.Log);

            var kernel = Kernel.Builder
            .WithLogger(ConsoleLogger.Log)
            .Configure(c => c.AddAzureOpenAIEmbeddingGenerationService("text-embedding-ada-002", "text-embedding-ada-002", Env.Var("AZURE_OPENAI_ENDPOINT"), Env.Var("AZURE_OPENAI_KEY")))
            //.Configure(c => c.AddOpenAIEmbeddingGenerationService("text-embedding-ada-002", "text-embedding-ada-002", Env.Var("OPENAI_API_KEY")))
            .WithMemoryStorage(memoryStore)
            .Build();

            kernel.Config.AddAzureOpenAITextCompletionService("text-davinci-003", "text-davinci-003", Env.Var("AZURE_OPENAI_ENDPOINT"), Env.Var("AZURE_OPENAI_KEY"));

            if(question) RunAsync(kernel).Wait();
            if(parse) ParseText(kernel, tenkfile).Wait();
        }

        static async Task ParseText(IKernel kernel, string kfile)
        {
            string text = File.ReadAllText(kfile);
            var allsentences = BlingFireUtils.GetSentences(text);

            var i = 0;
            foreach (var s in allsentences)
            {
                await kernel.Memory.SaveReferenceAsync(
                    collection: memoryCollectionName,
                    description: s,
                    text: s,
                    externalId: i.ToString(),
                    externalSourceName: "MS10-K"
                );
                Console.WriteLine($"  sentence {++i} saved");
            }
        }

        public static async Task RunAsync(IKernel kernel)
        {
            Console.WriteLine("\nHi, welcome to Microsoft's 2022 10-K. What would you like to know?\n");
            while (true)
            {
                Console.Write("User: ");
                var query = Console.ReadLine();
                if (query == null) { break; }
                var results = kernel.Memory.SearchAsync(memoryCollectionName, query, limit: 3, minRelevanceScore: 0.77);

                string FUNCTION_DEFINITION = "Act as the company Microsoft. Answer questions about your annual financial report. Only answer questions based on the info listed below. If the info below doesn't answer the question, say you don't know.\n[START INFO]\n";

                await foreach (MemoryQueryResult r in results)
                {
                    int id = int.Parse(r.Metadata.Id);
                    MemoryQueryResult rb2 = kernel.Memory.GetAsync(memoryCollectionName, (id - 2).ToString()).Result;
                    MemoryQueryResult rb = kernel.Memory.GetAsync(memoryCollectionName, (id - 1).ToString()).Result;
                    MemoryQueryResult ra = kernel.Memory.GetAsync(memoryCollectionName, (id + 1).ToString()).Result;
                    MemoryQueryResult ra2 = kernel.Memory.GetAsync(memoryCollectionName, (id + 2).ToString()).Result;

                    FUNCTION_DEFINITION += "\n " + rb2.Metadata.Id + ": " + rb2.Metadata.Description + "\n";
                    FUNCTION_DEFINITION += "\n " + rb.Metadata.Description + "\n";
                    FUNCTION_DEFINITION += "\n " + r.Metadata.Description + "\n";
                    FUNCTION_DEFINITION += "\n " + ra.Metadata.Description + "\n";
                    FUNCTION_DEFINITION += "\n " + ra2.Metadata.Id + ": " + ra2.Metadata.Description + "\n";
                }

                FUNCTION_DEFINITION += "[END INFO]\n" + query;

                //  Console.WriteLine(FUNCTION_DEFINITION + "\n\n");
                //  Console.WriteLine(FUNCTION_DEFINITION.Length);

                var answer = kernel.CreateSemanticFunction(FUNCTION_DEFINITION, maxTokens: 250, temperature: 0);

                var result = await answer.InvokeAsync();
                Console.WriteLine("\nMS10K: " + result.Result.Trim() + "\n");
            }
        }

    }
}
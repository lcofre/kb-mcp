import 'dotenv/config';
import { mastra } from './mastra';
import { InputOutputSchema } from './schema';

async function main() {
  const sampleInput = {
    "greetings": {
      "hello": {
        "value": "Hello",
        "comment": "A common greeting"
      },
      "welcome": {
        "value": "Welcome to our app!",
        "old_translation": "Bienvenido a nuestra aplicación!"
      }
    },
    "farewell": {
      "goodbye": {
        "value": "Goodbye",
        "comment": "Used when leaving"
      }
    }
  };

  const agent = mastra.getAgent('orchestratorAgent');

  const prompt = `
Please translate the following JSON to Spanish.
The general instructions are in 'instructions.md'.
The Spanish specific instructions are in 'es.md'.
Websites to use as a reference: ['https://www.example.com/es']

JSON to translate:
${JSON.stringify(sampleInput, null, 2)}
`;

  const response = await agent.generate(prompt, {
    // I need to pass the context to the tools, but the generate function
    // does not seem to have a way to do that directly.
    // The tools are called by the agent based on the prompt.
    // The orchestrator agent needs to be smart enough to call the tools with the right parameters.
    // I will adjust the orchestrator agent's instructions to be more specific about how to call the tools.
  });

  console.log(response.text);
}

main();

import { openai } from '@ai-sdk/openai';
import { Agent } from '@mastra/core/agent';

export const diffAgent = new Agent({
  name: 'diffAgent',
  instructions: `You are a diff agent.
  You will be given a JSON object with an "old_translation" and a "new_translation".
  Compare the two translations and append to the "explanation" field any differences found.
  Also, explain if the differences support a change to the new translation over the old one.
  Return the updated JSON object.
  `,
  model: openai('gpt-4o'),
});

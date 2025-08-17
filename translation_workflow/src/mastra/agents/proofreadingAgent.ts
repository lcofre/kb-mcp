import { openai } from '@ai-sdk/openai';
import { Agent } from '@mastra/core/agent';

export const proofreadingAgent = new Agent({
  name: 'proofreadingAgent',
  instructions: `You are a proofreading agent.
  You will be given a JSON object with a "new_translation".
  Check the "new_translation" for any semantic or syntactic errors, or anything that reads wrong in the target language.
  Correct the "new_translation" if necessary and add an "explanation" for the changes.
  Return the updated JSON object.
  `,
  model: openai('gpt-4o'),
});

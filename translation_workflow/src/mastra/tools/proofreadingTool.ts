import { createTool } from '@mastra/core/tools';
import { InputOutputSchema } from '../../schema';

export const proofreadingTool = createTool({
  id: 'proofreadingTool',
  description: 'Calls the proofreading agent to check the new translation.',
  inputSchema: InputOutputSchema,
  outputSchema: InputOutputSchema,
  execute: async ({ context, mastra }) => {
    const agent = mastra!.getAgent('proofreadingAgent');

    const result = await agent!.generate(JSON.stringify(context, null, 2));
    const updatedJson = JSON.parse(result.text);

    return updatedJson;
  },
});

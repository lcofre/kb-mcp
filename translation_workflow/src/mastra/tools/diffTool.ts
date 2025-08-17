import { createTool } from '@mastra/core/tools';
import { InputOutputSchema } from '../../schema';

export const diffTool = createTool({
  id: 'diffTool',
  description: 'Calls the diff agent to compare the old and new translations.',
  inputSchema: InputOutputSchema,
  outputSchema: InputOutputSchema,
  execute: async ({ context, mastra }) => {
    const agent = mastra!.getAgent('diffAgent');

    // We need to iterate over the entire structure and call the agent for each entry
    // that has both old_translation and new_translation.
    // However, the agent is designed to take the whole JSON.
    // A better approach would be for the agent to handle the iteration internally.
    // For now, I will just pass the whole JSON to the agent.

    const result = await agent!.generate(JSON.stringify(context, null, 2));
    const updatedJson = JSON.parse(result.text);

    return updatedJson;
  },
});

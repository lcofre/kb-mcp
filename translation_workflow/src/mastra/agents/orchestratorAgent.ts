import { openai } from '@ai-sdk/openai';
import { Agent } from '@mastra/core/agent';
import { translatorTool } from '../tools/translatorTool';
import { diffTool } from '../tools/diffTool';
import { proofreadingTool } from '../tools/proofreadingTool';

export const orchestratorAgent = new Agent({
  name: 'orchestratorAgent',
  instructions: `You are an orchestrator agent for a translation workflow.
You will be given a prompt containing a JSON object to translate, a target language, paths to instruction files, and reference websites.

Your tasks are:
1. **Parse the prompt** to extract the following information:
   - The JSON object to be translated.
   - The target language.
   - The path to the general instructions file.
   - The path to the language-specific instructions file.
   - A list of reference websites.

2. **Call the \`translatorTool\`** with the extracted information to get the initial translation. The output of this tool will be the translated JSON object.

3. **Iterate through the translated JSON object**. For each entry that has a non-empty \`old_translation\` field, **call the \`diffTool\`**. The input to the \`diffTool\` should be the entire JSON object from the previous step. The \`diffTool\` will add explanations about the differences.

4. **Call the \`proofreadingTool\`** with the JSON object from the previous step. This tool will check and correct the translations.

5. **Return the final, proofread JSON object** as a single JSON string. Do not include any other text in your response.
`,
  model: openai('gpt-4o'),
  tools: {
    translatorTool,
    diffTool,
    proofreadingTool,
  },
});

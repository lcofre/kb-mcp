import { createTool } from '@mastra/core/tools';
import { z } from 'zod';
import { InputOutputSchema } from '../../schema';

export const translatorTool = createTool({
  id: 'translatorTool',
  description: 'Calls the translator agent to translate the given JSON object.',
  inputSchema: z.object({
    json: InputOutputSchema,
    language: z.string(),
    generalInstructionsPath: z.string(),
    languageInstructionsPath: z.string(),
    websites: z.array(z.string()),
  }),
  outputSchema: InputOutputSchema,
  execute: async ({ context, mastra }) => {
    const { json, language, generalInstructionsPath, languageInstructionsPath, websites } = context;

    const agent = mastra!.getAgent('translatorAgent');

    const prompt = `
Translate the following JSON object to ${language}.
Input JSON:
${JSON.stringify(json, null, 2)}

Read the general instructions from ${generalInstructionsPath}.
Read the language specific instructions from ${languageInstructionsPath}.
Use the following websites for reference:
${websites.join('\n')}

Please return only the translated JSON object.
`;

    const result = await agent!.generate(prompt);
    const translatedJson = JSON.parse(result.text);

    return translatedJson;
  },
});

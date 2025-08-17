import { openai } from '@ai-sdk/openai';
import { Agent } from '@mastra/core/agent';
import { readFileTool } from '../tools/fs/readFileTool';
import { browseWebTool } from '../tools/web/browseWebTool';

export const translatorAgent = new Agent({
  name: 'translatorAgent',
  instructions: `You are a translator agent.
  You will be given a JSON object with strings to translate.
  You need to translate the "value" property of each entry to the target language.
  You should use the provided general and language-specific instructions.
  You can use the readFile tool to read instruction files and the browseWeb tool to find examples of translations.
  The output should be a JSON object with the "new_translation" field populated.
  You can also add an "explanation" field in English to explain your choices.
  `,
  model: openai('gpt-4o'),
  tools: {
    readFileTool,
    browseWebTool,
  },
});

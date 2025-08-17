import { createTool } from '@mastra/core/tools';
import { z } from 'zod';

export const browseWebTool = createTool({
  id: 'browseWeb',
  description: 'Browses a web page and returns its content.',
  inputSchema: z.object({
    url: z.string(),
  }),
  outputSchema: z.object({
    content: z.string(),
  }),
  execute: async ({ context }) => {
    const { url } = context;
    const response = await fetch(url);
    const content = await response.text();
    return { content };
  },
});

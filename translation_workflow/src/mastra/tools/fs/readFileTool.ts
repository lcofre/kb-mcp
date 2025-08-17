import { createTool } from '@mastra/core/tools';
import { z } from 'zod';
import * as fs from 'fs/promises';

export const readFileTool = createTool({
  id: 'readFile',
  description: 'Reads a file and returns its content.',
  inputSchema: z.object({
    path: z.string(),
  }),
  outputSchema: z.object({
    content: z.string(),
  }),
  execute: async ({ context }) => {
    const { path } = context;
    const content = await fs.readFile(path, 'utf-8');
    return { content };
  },
});

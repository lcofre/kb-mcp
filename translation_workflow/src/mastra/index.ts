import { Mastra } from '@mastra/core/mastra';
import { translatorAgent } from './agents/translatorAgent';
import { diffAgent } from './agents/diffAgent';
import { proofreadingAgent } from './agents/proofreadingAgent';
import { orchestratorAgent } from './agents/orchestratorAgent';
import { translatorTool } from './tools/translatorTool';
import { diffTool } from './tools/diffTool';
import { proofreadingTool } from './tools/proofreadingTool';
import { readFileTool } from './tools/fs/readFileTool';
import { browseWebTool } from './tools/web/browseWebTool';

export const mastra = new Mastra({
  agents: {
    translatorAgent,
    diffAgent,
    proofreadingAgent,
    orchestratorAgent,
  },
  tools: {
    translatorTool,
    diffTool,
    proofreadingTool,
    readFileTool,
    browseWebTool,
  }
});

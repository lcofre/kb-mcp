import { z } from "zod";

const TranslationSchema = z.object({
  old_translation: z.string(),
  new_translation: z.string().optional(),
  explanation: z.string().optional(),
});

const EntrySchema = z.object({
  value: z.string(),
  comment: z.string().optional(),
}).catchall(TranslationSchema);

// Each section has multiple entries
const SectionSchema = z.record(EntrySchema);

// Root object groups sections
export const InputOutputSchema = z.record(SectionSchema);

export const PROCESS_OPTIONS = [
  { value: 'blur', label: 'Simple Blur' },
  { value: 'gblur', label: 'Gaussian Blur' },
  { value: 'grayscale', label: 'Grayscale' },
  { value: 'adj_brightness', label: 'Adjust Brightness' },
  { value: 'simple_threshold', label: 'Simple Global Threshold' },
] as const;

export type ProcessOptionsValue = (typeof PROCESS_OPTIONS)[number]['value'];

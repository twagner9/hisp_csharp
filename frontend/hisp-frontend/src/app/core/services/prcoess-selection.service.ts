import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SelectedOpService {
  readonly userSelectedProcess = signal<string>('blur');
  // readonly formData = signal<FormData | null>(null);
  readonly image = signal<File | null>(null);
  readonly imageUploaded = signal<boolean>(false);
  readonly kernelVal = signal<string>('3');
  readonly sigmaVal = signal<string>('0.5');
}

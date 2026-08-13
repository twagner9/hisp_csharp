import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SelectedOpService {
  readonly userSelectedProcess = signal<string>('');
  readonly formData = signal<FormData | null>(null);
  readonly imageUploaded = signal<boolean>(false);
}

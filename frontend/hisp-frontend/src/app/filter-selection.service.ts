import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SelectedOpService {
  public readonly userSelectedProcess = signal<string>('');
}

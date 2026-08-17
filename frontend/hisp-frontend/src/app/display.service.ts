import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DisplayService {
  readonly processedImages = signal<string[]>([]);
  readonly mostRecentlyProcessedImg = signal<string>('');
}

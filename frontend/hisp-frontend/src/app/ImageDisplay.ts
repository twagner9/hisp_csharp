import { Component, signal } from '@angular/core';

@Component({
  selector: 'image-display',
  templateUrl: 'image-display.html',
})
export class ImageDisplay {
  displayedImage = signal<Blob | null>(null);
}

import { Component, signal } from '@angular/core';
import { DisplayService } from './display.service';

@Component({
  selector: 'image-display',
  templateUrl: 'image-display.html',
})
export class ImageDisplay {
  constructor(displaySvc: DisplayService) {}
  imgIdx = signal<number>(0);
}

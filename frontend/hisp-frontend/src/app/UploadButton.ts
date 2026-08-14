import { Component, signal } from '@angular/core';
import { ProcessSelection } from './ProcessSelection';
import { SelectedOpService } from './prcoess-selection.service';

@Component({
  selector: 'upload-button',
  templateUrl: 'upload-button.html',
})
export class UploadButton {
  fileContent: string = '';
  gettingImage = signal<boolean>(false);

  // Give access to the HttpClient for the whole class
  constructor(public svc: SelectedOpService) {}

  onFileSelected(event: Event) {
    this.gettingImage.set(true);
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length == 0) {
      this.gettingImage.set(false);
      return;
    }

    const file = input.files[0];

    console.log('File loaded: ', file.name);
    console.log('File type: ', file.type);
    console.log('File size: ', file.size);

    this.svc.formData.set(new FormData());
    this.svc.formData()?.append('image', file);
    this.svc.imageUploaded.set(true);
    this.gettingImage.set(false);
  }
}

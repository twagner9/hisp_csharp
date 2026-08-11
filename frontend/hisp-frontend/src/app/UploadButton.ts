import { Component, signal } from '@angular/core';

@Component({
  selector: 'upload-button',
  templateUrl: 'upload-button.html',
})
export class UploadButton {
  fileContent: string = '';
  gettingImage = signal<boolean>(false);

  onFileSelected(event: Event) {
    this.gettingImage.set(true);
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length == 0) {
      return;
    }

    const file = input.files[0];

    console.log('File loaded: ', file.name);
    console.log('File type: ', file.type);
    console.log('File size: ', file.size);
  }
}

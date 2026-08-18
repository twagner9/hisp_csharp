import { Component, signal } from '@angular/core';
import { ProcessSelection } from '../process-selection/ProcessSelection';
import { SelectedOpService } from '../../core/services/prcoess-selection.service';
import { DisplayService } from '../../core/services/display.service';

@Component({
  selector: 'upload-button',
  templateUrl: 'upload-button.html',
})
export class UploadButton {
  fileContent: string = '';
  gettingImage = signal<boolean>(false);

  // Give access to the HttpClient for the whole class
  constructor(
    public selectionSvc: SelectedOpService,
    public displaySvc: DisplayService,
  ) {}

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

    this.selectionSvc.formData.set(new FormData());
    this.selectionSvc.formData()?.append('image', file);
    this.displaySvc.uploadedImg.set(URL.createObjectURL(file));

    this.selectionSvc.imageUploaded.set(true);
    this.gettingImage.set(false);
  }
}

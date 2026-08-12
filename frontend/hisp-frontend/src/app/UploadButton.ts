import { Component, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'upload-button',
  templateUrl: 'upload-button.html',
})
export class UploadButton {
  fileContent: string = '';
  gettingImage = signal<boolean>(false);

  // Give access to the HttpClient for the whole class
  constructor(private http: HttpClient) {}

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

    const formData = new FormData();
    formData.append('image', file);

    // Send the selected image to the backend for processing
    this.http
      .post<{ fileName: string }>('http://localhost:5192/api/Image/process', formData)
      .subscribe({
        next: (response) => {
          console.log('Backend received', response.fileName);
          this.gettingImage.set(false);
        },
        error: (error) => {
          console.error('Upload failed:', error);
          this.gettingImage.set(false);
        },
      });
  }
}

import { Component, computed, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SelectedOpService } from './prcoess-selection.service';

@Component({
  selector: 'submit-button',
  template: `<button (click)="submitProcessingJob()" [disabled]="!readyForSubmission()">
    Submit
  </button>`,
})
export class SubmitButton {
  readyForSubmission = computed(() => {
    const data = this.svc.formData();
    return data !== null && data !== undefined;
  });

  constructor(
    public svc: SelectedOpService,
    private http: HttpClient,
  ) {}
  submitProcessingJob() {
    // Pass the userSelectedProcess to the backend along with the image data
    console.log(`submitProcessingJob() has ${this.svc.userSelectedProcess()}`);
    this.http
      .post(
        `http://localhost:5192/api/Image/process/${this.svc.userSelectedProcess()}`,
        this.svc.formData(),
        {
          responseType: 'blob',
        },
      )
      .subscribe({
        next: (response) => {
          console.log('Backend received', response);
          console.log('MIME type:', response.type);
          console.log('Size: ', response.size, 'bytes');
        },
        error: (error) => {
          console.error('Upload failed when submitting processing job:', error);
        },
        complete: () => {
          console.log('Successfully transmitted.');
        },
      });
  }
}

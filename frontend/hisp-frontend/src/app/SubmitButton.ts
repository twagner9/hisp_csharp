import { Component, computed, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SelectedOpService } from './prcoess-selection.service';
import { DisplayService } from './display.service';

@Component({
  selector: 'submit-button',
  template: `<button (click)="submitProcessingJob()" [disabled]="!readyForSubmission()">
    Submit
  </button>`,
})
export class SubmitButton {
  readyForSubmission = computed(() => {
    const data = this.selectionSvc.formData();
    return data !== null && data !== undefined;
  });

  constructor(
    public selectionSvc: SelectedOpService,
    public displaySvc: DisplayService,
    private http: HttpClient,
  ) {}
  submitProcessingJob() {
    // Pass the userSelectedProcess to the backend along with the image data
    console.log(`submitProcessingJob() has ${this.selectionSvc.userSelectedProcess()}`);
    this.http
      .post(
        `http://localhost:5192/api/Image/process/${this.selectionSvc.userSelectedProcess()}`,
        this.selectionSvc.formData(),
        {
          responseType: 'blob',
        },
      )
      .subscribe({
        next: (blob) => {
          console.log('Backend received', blob);
          console.log('MIME type:', blob.type);
          console.log('Size: ', blob.size, 'bytes');
          const imageUrl = URL.createObjectURL(blob);
          this.displaySvc.mostRecentlyProcessedImg.set(imageUrl);
          this.displaySvc.processedImages.update((images) => [...images, imageUrl]);
          console.log('Num processed imgs: ', this.displaySvc.processedImages().length);
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

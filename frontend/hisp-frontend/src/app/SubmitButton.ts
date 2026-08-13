import { Component, signal, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SelectedOpService } from './filter-selection.service';

@Component({
  selector: 'submit-button',
  template: `<button (onclick)="submitProcessingJob()">Submit</button>`,
})
export class SubmitButton {
	readyForSubmission = signal<boolean>(false);

  constructor(
    public svc: SelectedOpService,
    private http: HttpClient,
  ) {
		effect(() => {
			if (this.svc.formData() !== null) {
				this.readyForSubmission.set(true);
			}
		});
	}
  submitProcessingJob() {
    // Pass the userSelectedProcess to the backend along with the image data
    this.http
      .post<{ fileName: string }>(
        `http://localhost:5192/api/Image/process/${this.svc.userSelectedProcess()}`,
        this.svc.formData(),
      )
      .subscribe({
        next: (response) => {
          console.log('Backend received', response.fileName);
        },
        error: (error) => {
          console.error('Upload failed:', error);
        },
      });
    this.svc.userSelectedProcess();
  }
}

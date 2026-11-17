import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UploadButton } from './upload-button';

describe('image-upload', () => {
  let component: UploadButton;
  let fixture: ComponentFixture<UploadButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UploadButton],
    }).compileComponents();

    fixture = TestBed.createComponent(UploadButton);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have <input> element with type file', () => {
    const uploadButtonElement = fixture.nativeElement;
    const inputElement = uploadButtonElement.querySelector('input');

    expect(inputElement).toBeTruthy();
    expect(inputElement.getAttribute('type')).toBe('file');
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditableText } from './editable-text';
import { NameText } from './name-text';

describe('EditableText', () => {
  let editableComponent: EditableText;
  let editableFixture: ComponentFixture<EditableText>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditableText],
    }).compileComponents();

    editableFixture = TestBed.createComponent(EditableText);
    editableComponent = editableFixture.componentInstance;
    await editableFixture.whenStable();
  });

  it('should create', () => {
    expect(editableComponent).toBeTruthy();
  });

  it('should have span not input on init render', () => {
    const editableText = editableFixture.nativeElement;
    let inputElement = editableText.querySelector('input');
    let spanElement = editableText.querySelector('span.autoWidth');

    expect(inputElement).toBeNull();
    expect(spanElement).toBeTruthy();
    expect(spanElement.textContent).toBe(''); // Should be empty on initial, I believe?
  });

  it('should have input after click event', () => {
    const editableText = editableFixture.nativeElement;
    let clickableSpan: HTMLSpanElement = editableText.querySelector(
      'span.autoWidth',
    ) as HTMLSpanElement;

    clickableSpan.dispatchEvent(new Event('click'));
    editableFixture.detectChanges();
    clickableSpan = editableText.querySelector('span.autoWidth');
    let input: HTMLInputElement = editableText.querySelector('input.autoWidth') as HTMLInputElement;

    expect(input).toBeTruthy();
    expect(clickableSpan).toBeFalsy();

    // Also return the span after confirming change
    // a. Do event
    input.value = 'Jenna';
    input.dispatchEvent(new Event('input'));
    input.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter' }));
    editableFixture.detectChanges();

    // b. Check result
    clickableSpan = editableText.querySelector('span.autoWidth');
    input = editableText.querySelector('input.autoWidth');
    expect(clickableSpan.textContent.trim()).toBe('Jenna');
    expect(input).toBeFalsy();
  });
});

describe('NameText', () => {
  let nameComponent: NameText;
  let nameFixture: ComponentFixture<NameText>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NameText],
    }).compileComponents();

    nameFixture = TestBed.createComponent(NameText);
    nameComponent = nameFixture.componentInstance;
    await nameFixture.whenStable();
  });

  it('should create', () => {
    expect(nameComponent).toBeTruthy();
  });

  it('should have default text', () => {
    const headingElement = nameFixture.nativeElement;
    expect(headingElement.textContent.trim()).toBe('Hello Anonymous, ready to process images?');
  });

  it('should update text after user interaction', () => {
    let headingElement: HTMLHeadingElement = nameFixture.nativeElement.querySelector('h1');
    let editableElement = nameFixture.nativeElement.querySelector('editable-text');

    expect(headingElement.textContent.trim()).toBe('Hello Anonymous, ready to process images?');

    const spanElement = editableElement.querySelector('span.autoWidth');
    spanElement.dispatchEvent(new Event('click'));
    nameFixture.detectChanges();

    const input = editableElement.querySelector('input.autoWidth');
    input.value = 'Jenna';
    input.dispatchEvent(new Event('input'));
    input.dispatchEvent(new KeyboardEvent('keydown', { key: 'Enter' }));
    nameFixture.detectChanges();
    headingElement = nameFixture.nativeElement.querySelector('h1');
    expect(headingElement.textContent.trim()).toBe('Hello Jenna, ready to process images?');
  });
});

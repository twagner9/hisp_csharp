import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProcessSelection } from './process-selection';
import { PROCESS_OPTIONS } from './ProcessOptions';

describe('ProcessSelection', () => {
  let component: ProcessSelection;
  let fixture: ComponentFixture<ProcessSelection>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProcessSelection],
    }).compileComponents();

    fixture = TestBed.createComponent(ProcessSelection);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  // Will ensure the element renders all options.
  it('should render option for each image processing operation', () => {
    const processSelectionElement = fixture.nativeElement;
    const options = processSelectionElement.querySelectorAll('option');

    expect(options.length).toBe(PROCESS_OPTIONS.length);

    PROCESS_OPTIONS.forEach((opt, i) => {
      expect(options[i].getAttribute('value')).toBe(opt.value);
      expect(options[i].textContent).toContain(opt.label);
    });
  });

  // Will ensure no options are removed that should still be present.
  it('should contain the core processes', () => {
    const values = PROCESS_OPTIONS.map((o) => o.value);
    expect(values).toContain('blur');
    expect(values).toContain('gblur');
    expect(values).toContain('grayscale');
    expect(values).toContain('adj_brightness');
    expect(values).toContain('simple_threshold');
  });

  it('should display only needed inputs', () => {
    const processSelectionElement: HTMLElement = fixture.nativeElement as HTMLElement;

    let visible = Array.from(
      processSelectionElement.querySelectorAll<HTMLElement>('.additionalData:not(.hidden)'),
    ).flatMap((d) => d.dataset['param']?.split(' ') ?? []);

    expect(visible[0]).toEqual('blurs');

    // Update the selected option
    const select: HTMLSelectElement = processSelectionElement.querySelector(
      'select.mainSelector',
    ) as HTMLSelectElement;
    select.selectedIndex = 1;
    select.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    visible = Array.from(
      processSelectionElement.querySelectorAll<HTMLElement>('.additionalData:not(.hidden)'),
    ).flatMap((d) => d.dataset['param']?.split(' ') ?? []);

    expect(visible.length).toBe(2);
    expect(visible[0]).toBe('blurs');
    expect(visible[1]).toBe('gblur');

    // Check idx 2
    select.selectedIndex = 2;
    select.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    visible = Array.from(
      processSelectionElement.querySelectorAll<HTMLElement>('.additionalData:not(.hidden)'),
    ).flatMap((d) => d.dataset['param']?.split(' ') ?? []);

    expect(visible.length).toBe(0);
    expect(visible[0]).toBe(undefined); // undefined because grayscale has no additional inputs!

    select.selectedIndex = 3;
    select.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    visible = Array.from(
      processSelectionElement.querySelectorAll<HTMLElement>('.additionalData:not(.hidden)'),
    ).flatMap((d) => d.dataset['param']?.split(' ') ?? []);

    expect(visible.length).toBe(1);
    expect(visible[0]).toBe('adj_brightness');

    select.selectedIndex = 4;
    select.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    visible = Array.from(
      processSelectionElement.querySelectorAll<HTMLElement>('.additionalData:not(.hidden)'),
    ).flatMap((d) => d.dataset['param']?.split(' ') ?? []);

    expect(visible.length).toBe(1);
    expect(visible[0]).toBe('simple_threshold');
  });
});

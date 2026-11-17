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
    const values = PROCESS_OPTIONS.map(o => o.value);
    expect(values).toContain('blur');
    expect(values).toContain('gblur');
    expect(values).toContain('grayscale');
    expect(values).toContain('adj_brightness');
    expect(values).toContain('simple_threshold');
  });

  it('should display only needed inputs', () => {
    const processSelectionElement = fixture.nativeElement as HTMLElement;
    const visible = Array.from(processSelectionElement.querySelectorAll<HTMLElement>('.additionalData:not(.hidden)')).flatMap(d => d.dataset['param']?.split(' ')?? []);

    expect(visible[0]).toEqual('blur');
  });
});

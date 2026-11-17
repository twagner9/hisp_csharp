import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ImageDisplay } from './image-display';

/**
 * Alternative to beforeEach that can be called in every test; advantageous for having customizable params
 */
// function setup(providers?: Provider[]): ComponentFixture<ImageDisplay>) {
//   TestBed.configureTestingModule({ providers });
//   return TestBed.createComponent(ImageDisplay);
// }

describe('image-display', () => {
  let component: ImageDisplay;
  let fixture: ComponentFixture<ImageDisplay>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImageDisplay],
    }).compileComponents();

    // DO NOT reconfigure TestBed after calling createComponent -- presumably causes issues and will error out
    // The fixture here is used as a harness for interacting with the created component, so the component is accessed through it.
    fixture = TestBed.createComponent(ImageDisplay); // Adds element to test DOM and returns a fixture of the component
    component = fixture.componentInstance;
    await fixture.whenStable(); // Makes sure component is fully rendered
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have <img> element', () => {
    const imageDisplayElement: HTMLElement = fixture.nativeElement;
    const img = imageDisplayElement.querySelector('img');
    const arrowDiv = imageDisplayElement.querySelector('div.imageArea');

    expect(img).toBeTruthy();
    expect(img?.getAttribute('src')).toBe(''); // default the <img> src should be empty string -- see display.service 
  });

  it('should not have arrowDiv', () => {
    const imageDisplayElement: HTMLElement = fixture.nativeElement
    const arrowDiv = imageDisplayElement.querySelector('div.resultImageArea');
    expect(arrowDiv).toBeNull();
  });
});

import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CobrancasTableComponent } from './cobrancas-table.component';

describe('CobrancasTableComponent', () => {
  let component: CobrancasTableComponent;
  let fixture: ComponentFixture<CobrancasTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CobrancasTableComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CobrancasTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

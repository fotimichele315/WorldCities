
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; // ⭐ Import fondamentale
import { AngularMaterialModule } from '../angular-material.module';
import { RouterTestingModule } from '@angular/router/testing';
import { CitiesComponent } from '../cities/cities.component';
import { City } from './city';
import { CityService } from './city.service';
import { ApiResult } from '../base.service';
import { of } from 'rxjs';

describe('CitiesComponent', () => {
  let cityService: jasmine.SpyObj<CityService>;
  let component: CitiesComponent;
  let fixture: ComponentFixture<CitiesComponent>;

  beforeEach(async () => {

    cityService = jasmine.createSpyObj<CityService>('CityService', ['getData']);

    const mockResult: ApiResult<City> = {
      data: [
        {
          name: 'TestCity1',
          id: 1,
          lat: 1,
          lon: 1,
          population: 1,
          countryId: 1,
          countryName: 'TestCountry1'
        }
      ],
      totalCount: 1,
      pageIndex: 0,
      pageSize: 10,
      totalPages: 1,
      sortColumn: '',
      sortOrder: '',
      filterColumn: '',
      filterQuery: ''
    };

    cityService.getData.and.returnValue(of(mockResult));

    await TestBed.configureTestingModule({
      declarations: [CitiesComponent],
      imports: [
        BrowserAnimationsModule,
        AngularMaterialModule,
        RouterTestingModule
      ],
      providers: [
        { provide: CityService, useValue: cityService } // ✅ GIUSTO
      ]
    }).compileComponents();
  });

  beforeEach(() => {

    fixture = TestBed.createComponent(CitiesComponent);
    component = fixture.componentInstance;

    //TODO: configure fixture/component/children/etc.
    component.paginator = {
      length: 3,
      pageIndex: 0,
      pageSize: 10
    } as any;


    fixture.detectChanges();


  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  //TODO: implement some other tests
  it('should display a "Cities" title', () => {
    let title = fixture.nativeElement.querySelector('h1');
    expect(title.textContent).toEqual('Cities');
  });

  it('should contain a table with one or more cities', () => {
    const table = fixture.nativeElement.querySelector('table.mat-mdc-table');
    expect(table).toBeTruthy(); // la tabella esiste

    const tableRows = table.querySelectorAll('tr.mat-mdc-row'); // querySelectorAll ritorna NodeList
    expect(tableRows.length).toBeGreaterThan(0); // almeno una riga
  });


});

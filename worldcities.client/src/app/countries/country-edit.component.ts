import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder , FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { environment } from './../../environments/environment';
 import { Country } from './../countries/country';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
@Component({
  selector: 'app-country-edit',
  standalone: false,
  templateUrl: './country-edit.component.html',
  styleUrl: './country-edit.component.scss'
})
export class CountryEditComponent implements OnInit {

  // the view title
  title?: string;

  //the form model
  form!: FormGroup;

  // the city object to edit or create
  country?: Country;


  // the city object id, as fetched fron the active route:
  // It's NULL when we're adding a new city, and not NULL when we're editing an existing one
  id?: number;




  constructor(
    private fb: FormBuilder,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) { }


  ngOnInit() {

    this.form = this.fb.group({

      name: [
        '',
        [Validators.required],               // <-- sync
        [this.isDupeField("name")]           // <-- async
      ],

      iso2: [
        '',
        [Validators.required, Validators.pattern(/^[a-zA-z]{2}$/)], // <-- sync
        [this.isDupeField("iso2")]                               // <-- async
      ],

      iso3: [
        '',
        [Validators.required, Validators.pattern(/^[a-zA-z]{3}$/)], // <-- sync
        [this.isDupeField("iso3")]                               // <-- async
      ]
    });

    this.loadData();
  }

  loadData() {

 
    // retrive the ID from the 'id' parameter
    var idParam = this.activatedRoute.snapshot.paramMap.get('id');
    this.id = idParam ? +idParam : 0;
    if (this.id) {
      // EDIT MODE

      //fetch the country from the server
      var url = environment.baseUrl + 'api/Countries/' + this.id;
      this.http.get<Country>(url).subscribe({
        next: (result) => {
          this.country = result;
          this.title = "Edit - " + this.country!.name;
          console.log(this.country)
          // update the form with the country value
          this.form.patchValue(this.country!);
        },
        error: (error) => console.error("error" + error),
      })

    } else {
      // ADD NEW MODE
      this.title = "Create a new country";
    }
  }

 

  onSubmit() {
    var country = (this.id) ? this.country : <Country>{};
    if (country) {
      country.name = this.form.controls['name'].value;
      country.iso2 = this.form.controls['iso2'].value;
      country.iso3 = this.form.controls['iso3'].value;

    }

    if (this.id) {
      // EDIT MODE
      var url = environment.baseUrl + 'api/Countries/' + country!.id;
      this.http.put<Country>(url, country).subscribe({
        next: (result) => {
          console.log("Country " + country!.id + " has been updated")

          // go back to Countries views
          this.router.navigate(['/Countries']);
        },
        error: (error) => console.error("error" + error),
      })
    } else {
      // ADD NEW MODE
      var url = environment.baseUrl + 'api/Countries';
      this.http.post<Country>(url, country).subscribe({
        next: (result) => {
          console.log("Countries " + result.id + " has been created")

          // go back to countries views
          this.router.navigate(['/countries']);
        },
        error: (error) => console.error("error" + error),
      })
    }
  }

  isDupeField(fieldName: string): AsyncValidatorFn {

    return (control: AbstractControl): Observable<{
      [key: string]: any
    } | null> => {

      var params = new HttpParams()
        .set("countryId", (this.id) ? this.id.toString() : "0")
        .set("fieldName", fieldName)
        .set("fieldValue", control.value)

      var url = environment.baseUrl + 'api/Countries/IsDupeField';
      return this.http.post<boolean>(url, null, { params }).pipe(map(result => {
        return (result ? { isDupeField: true } : null);

      }));

    }
  }

}

import { Component, OnInit, ViewChild } from '@angular/core';
//import { HttpClient, HttpParams } from '@angular/common/http';
//import { environment } from './../../environments/environment';
import { City } from './city';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { CityService } from './city.service';
 @Component({
  selector: 'app-cities',
  standalone: false,
  templateUrl: './cities.component.html',
  styleUrl: './cities.component.scss'
})
export class CitiesComponent implements OnInit {

  filterTextChanged: Subject<string> = new Subject<string>(); 

  public cities!: MatTableDataSource<City>;

  defaultPageIndex = 0;
  defaultPageSize = 10;
  public defaultSortColumn:string = "name";
  public defaultSortOrder: "asc" | "desc" = "asc";
  defaultFilterColumn: string = "name";
  filterQuery: string|undefined ="";



  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  public displayedColumns: string[] = ['id', 'name', 'lat', 'lon', 'population', 'countryName']

   constructor(private cityService: CityService ) {

  }

  ngOnInit() {
    this.loadData();
  }

  onFilterTextChanged(filterText:string) {

    if (!this.filterTextChanged.observed) {
      this.filterTextChanged
        .pipe(debounceTime(1000), distinctUntilChanged())
        .subscribe(query => {
          this.loadData(query);
        }
        );
    }
    this.filterTextChanged.next(filterText);

  }
  


  loadData(query?:string) {
  var pageEvent = new PageEvent();
    pageEvent.pageIndex = 0;
    pageEvent.pageSize = 10;
    this.filterQuery = query;
    this.getData(pageEvent);
  }


  getData(event: PageEvent) {

    var sortColumn = (this.sort)
      ? this.sort.active
      : this.defaultSortColumn;

    var sortOrder = (this.sort)
      ? this.sort.direction
      : this.defaultSortOrder;

    var filterColumn = (this.filterQuery)
      ? this.defaultFilterColumn
      : null;

    var filterQuery = (this.filterQuery)
      ? this.filterQuery
      : null;

    this.cityService.getData(
      event.pageIndex,
      event.pageSize,
      sortColumn,
      sortOrder,
      filterColumn,
      filterQuery
    ).subscribe({
      next: (result) => {
         this.paginator.length = result.totalCount;
        this.paginator.pageIndex = result.pageIndex;
        this.paginator.pageSize = result.pageSize;
        this.cities = new MatTableDataSource<City>(result.data);
      },
      error: (error) => console.error("error" + error),
    })  }
  }


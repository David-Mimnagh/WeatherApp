import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { fromEvent } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-weather-data',
    templateUrl: './weather-data.component.html',
    styleUrls: ['./weather-data.component.css']
})
export class WeatherDataComponent {
    public forecast: WeatherForecast;
    public citiesList: CityCondensed[];
    httpClient: HttpClient;
    baseUrl: string;
    loading: boolean;
    selectedCity: boolean;
    useMetric: boolean;
    @ViewChild('cityInput', { static: false }) locationSearchBox: ElementRef;
    ngAfterViewInit() {
        fromEvent(this.locationSearchBox.nativeElement, 'keyup').pipe(
            debounceTime(2000) // 2 seconds
        ).subscribe((ev: HTMLInputElement) => {
            if (!this.selectedCity) {
                //@ts-ignore
                this.getCities(ev.target.value)
            }
            //resolve an issue where the selection of a city fired the debounce.
            this.selectedCity = false;
        });
        const input = document.querySelector('#locationSearchInput')
        //@ts-ignore
        input.onchange = (e) => {
            this.selectedCity = true;
            var opts = document.getElementById('city-list').childNodes;
            for (var i = 0; i < opts.length; i++) {
                //@ts-ignore
                if (opts[i].value === e.target.value) {
                    //@ts-ignore
                    this.getWeatherForecast(null, null, opts[i].innerText);
                    break;
                }
            }
        }
    }
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
      this.httpClient = http;
      this.baseUrl = baseUrl;
      this.loading = false;
      this.selectedCity = false;
      this.useMetric = false;
    }
    getWeatherForecast(latitude: number = null, longitude: number = null, cityKey: string = null) {
        if (latitude && !longitude || !latitude && longitude) {
            throw new Error("Please provide both a latitute and longitude.")
        }
        let queryParams = new HttpParams();

        if (latitude && longitude) {
            queryParams = queryParams.append("latitude", latitude.toString());
            queryParams = queryParams.append("longitude", longitude.toString());
        }

        if (location) {
            queryParams = queryParams.append("cityKey", cityKey);
        }
        queryParams = queryParams.append("useMetric", this.useMetric.toString());
        this.loading = true;
        this.httpClient.get<WeatherForecast>(this.baseUrl + 'WeatherForecast/GetWeatherInfo', { params: queryParams }).subscribe(result => {
            this.forecast = result;
            this.loading = false;
        }, error => console.error(error));
    }
    getCurrentLocation() {
        navigator.geolocation.getCurrentPosition((geoLoc) => this.getWeatherForecast(geoLoc.coords.latitude, geoLoc.coords.longitude)) 
    }
    getCities(location: string = null) {
        this.httpClient.get<CityCondensed[]>(this.baseUrl + 'City/GetCities', { params: { location } }).subscribe(result => {
            this.citiesList = result;
        }, error => console.error(error));
    }
    changeMetric(newValue: boolean) {
        this.useMetric = newValue;
    }
}
interface WeatherForecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
interface CityCondensed {
    Key: string;
    Name: number;
}

import { HttpClient, HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, firstValueFrom, map, of } from "rxjs";
import { ApiResponse } from "./apiResponse";

export class ApiBase {
  private http: HttpClient = inject(HttpClient);

  protected async get<T>(url: string): Promise<T | null> {
    return await firstValueFrom(this.http.get<ApiResponse<T>>(url, { observe: "response" })
      .pipe(
        map((response: HttpResponse<ApiResponse<T>>) => {
          if (!response.body) {
            console.error("Api error. Api returned empty body.");
            return null;
          }
          if (response.body.success === false) {
            console.error(`Api error. Status: ${response.status}, Error: ${response.body?.error}`);
            return null;
          }

          return response.body.result;
        }),

        catchError((error: HttpErrorResponse) => {
           console.error(`HTTP error ${error.status}:`, error.error ?? error.message);
           return of(null);
         })
      ));
  }
}

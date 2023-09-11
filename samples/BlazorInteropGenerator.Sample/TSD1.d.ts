/** interface comment */
export interface SomeType {
    /** my comment
     * multi line
    */
    prop1?: string;

    prop2: string[];

    prop3: number;

    prop4: any;

    prop5: boolean;

    prop6: string;

    method?(): string;

    /** my comment */
    method2(prop: string, prop2: number): string;

    prop7: INewInt;
}

/** nested
*/
export interface INewInt {
    prop1?: INewInt2;
}

export interface INewInt2 {
    prop1?: string;
}
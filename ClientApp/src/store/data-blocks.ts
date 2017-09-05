// Non-Action Data Blocks
export interface Company {
    symbol: string,
    name: string
}

export interface Graph {
    index: number
    graphId: string,
    company: Company,
    dataset: Array<number>,
    timeInterval: string
}



import { createStore, Reducer } from "redux";

export interface IState {
    counter: number
}

const INC = "INCREMENT";
const DEC = "DECREMENT";

interface Increment { type: "INCREMENT" };
interface Decrement { type: "DECREMENT" };

type CountAction = Increment | Decrement;

export const actions = {
    increment: {type: INC} as Increment,
    decrement: {type: DEC} as Decrement
};


function reducer(state: IState, action: CountAction): IState {
    switch (action.type) {
        case INC:
            return Object.assign({}, state, { counter: state.counter + 1 });
        case DEC:
            return Object.assign({}, state, { counter: state.counter -1 });
        default:
            return state;
    }
}

const initialState: IState = {
    counter: 0
};

export default createStore(reducer, initialState);

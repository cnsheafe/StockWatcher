import * as React from "react";
import {toggleModalDisplay} from "../store/actions";
import store, {IState} from "../store/store";
import { connect } from "react-redux";

interface ModalProps {
    showModal: boolean
}

class Modal extends React.Component<ModalProps, {}> {

    render() {
        return (
            <div id="modal" className={this.props.showModal ? "modal" : "hidden"}>
                <form className="modal-form">
                    <label>Price
                        <input type="text" className="modal-price"/>
                    </label>
                    <label htmlFor="">Phone Number
                        <input type="tel" className="modal-phone"/>
                    </label>
                    <button type="submit" className="modal-button">Submit</button>
                </form>
            </div>
        )
    }

    componentDidUpdate() {
        const mainElement = document.getElementsByTagName("main")[0];
        const callback = (event:KeyboardEvent) => {
            // Look for ESC Key
            if(event.which === 27) {
                store.dispatch(toggleModalDisplay());
            }
        }

        if (this.props.showModal) {
            mainElement.addEventListener("keyup", callback);
        }
        else {
            mainElement.removeEventListener("keyup", callback);
        }
    }
}

function mapStateToProps(state: IState) {
    return {
        showModal: state.showModal
    }
}

export default connect(mapStateToProps)(Modal);
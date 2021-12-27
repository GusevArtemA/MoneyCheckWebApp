import * as React from "react";

import "../../assets/scss/buttons/basic-button-assets.scss";
import "../../assets/scss/buttons/solid-button-style.scss";

export class SolidButton extends React.Component {
    render() {
        return (
            <button onClick={(e) => {
                var target = e.target;

                if(!target.className.startsWith('base-button')) {
                    while (!target.className.startsWith('base-button')) {
                        target = target.parentNode;
                    }
                }

                const cachedClassname = target.className;

                let animClassname = cachedClassname + ' wave-button';

                target.className = animClassname;

                setTimeout(() => {
                    console.log("A");
                    target.className = cachedClassname.replace('wave-button', '');
                }, 300);
            }} className={`base-button d-flex justify-content-center align-items-center filled-${this.props.fillColor ?? 'green'}`}>
                {this.props.children}
            </button>
        );
    }
}

import React from 'react';
import styles from './index.css';

export class ReadMore extends React.Component {
  constructor(props) {
    super(props);
    this.state = {expanded: false};
    this.handleClick = this.handleClick.bind(this);
  }

  handleClick() {
    this.setState({expanded: !this.state.expanded});
  }

  text() {
    return this.props.children || "-";
  }

  getDefaultLength() {
    if(this.text() instanceof Array) {
      return 2;
    } else {
      return 50;
    }
  }

  length() {
    return this.props.length || this.getDefaultLength();
  }

  shortText() {
    if(this.text() instanceof Array) {
      return this.text().slice(0, this.length());
    } else {
      return this.text().substring(0, this.length());
    }
  }

  render() {
    return (<div className={styles.breakWord}>
      <span>{this.state.expanded ? this.text() : this.shortText()}</span>
      {
        (() => {
          if(this.text().length > this.length()) {
            return <a className={styles.link} onClick={() => {this.handleClick()}}>
              { this.state.expanded ? " Show Less" : " Show More" }
            </a>
          }
        })()
      }
    </div>);
  }
}

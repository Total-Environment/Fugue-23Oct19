import React from 'react';
import styles from './index.css';
import classNames from 'classnames';

const componentTypes = {'material': 'Material', 'service': 'Service'};

export class Rate extends React.Component {
  renderLocationBasedRate(locationBasedRate) {
    return (
      <div className={classNames(this.props.rowStyle, styles.row)} key={locationBasedRate.typeOfPurchase}>
        <div className={classNames(this.props.columnStyle, styles.column, styles.left)}>{locationBasedRate.typeOfPurchase}</div>
        <div className={classNames(this.props.columnStyle, styles.column, styles.right)}>{locationBasedRate.controlBaseRate.value.toFixed(2)} {locationBasedRate.controlBaseRate.currency}</div>
        <div className={classNames(this.props.columnStyle, styles.column, styles.right)}>{locationBasedRate.landedRate.value.toFixed(2)} {locationBasedRate.controlBaseRate.currency}</div>
      </div>
    );
  }

  renderLocationBasedRates(locationBasedRates) {
    const location = locationBasedRates[0].location;
    return <div>
      <h4 className={styles.rateLocation}>{location}</h4>
      <div className={classNames(this.props.rowStyle, styles.row, styles.header)}>
        <div className={classNames(this.props.columnStyle, styles.column, styles.left)}>Mode of Purchase</div>
        <div className={classNames(this.props.columnStyle, styles.column, styles.right)}>Control Base Rate</div>
        <div className={classNames(this.props.columnStyle, styles.column, styles.right)}>Landed Rate</div>
      </div>
      {Object.keys(locationBasedRates).map(r => this.renderLocationBasedRate(locationBasedRates[r]))}
    </div>;
  }

  render() {
    if (this.props.rates && this.props.rates.length > 0) {
      const locationBasedRates = this.props.rates.reduce((obj, e) => {
        obj[e.location] = obj[e.location] || [];
        obj[e.location].push(e);
        return obj;
      }, {});
      return (<div className={styles.rate}>
        {
          Object.keys(locationBasedRates).map(r => this.renderLocationBasedRates(locationBasedRates[r]))
        }
      </div>);
    }
    else if (this.props.rates && this.props.rates.message) {
      return (<h3 className={styles.errorMessage}>{this.props.rates.message}</h3>);
    }
    else {
      return (<h3 className={styles.errorMessage}>{`Rate for ${componentTypes[this.props.componentType]} ${this.props.componentCode} not available for the current date. Please setup rate through Rate History page.`}</h3>);
    }
  }
}

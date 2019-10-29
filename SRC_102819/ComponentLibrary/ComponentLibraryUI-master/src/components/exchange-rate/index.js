import React from 'react';
import styles from './index.css';
import {Loading} from '../loading';
import classNames from 'classnames';
import {Link} from 'react-router';
import {PermissionedForNonComponents} from "../../permissions/permissions";
import {getViewExchangeRateHistoryPermissions} from "../../permissions/ComponentPermissions";

export class ExchangeRate extends React.Component {
  constructor(props) {
    super(props);
    this.renderHeader = this.renderHeader.bind(this);
    this.renderExchangeRate = this.renderExchangeRate.bind(this);
    this.renderDate = this.renderDate.bind(this);
    this.renderCurrencyINR = this.renderCurrencyINR.bind(this);
  }

  componentDidMount() {
    if (!this.props.exchangeRates) {
      this.props.onExchangeRatesFetchRequest();
    }
  }

  componentWillUnmount() {
    this.props.onExchangeRatesDestroy();
  }

  renderHeader() {
    return (<thead>
    <tr className={styles.tableHeader}>
      <th className={styles.text}>Currency</th>
      <th className={styles.number}>Conversion Rate</th>
      <th className={styles.number}>Currency Fluctuation Coefficient (%)</th>
      <th className={styles.number}>Defined Conversion Rate</th>
      <th className={styles.number}>Applied From</th>
    </tr>
    </thead>);
  }

  renderExchangeRate(exchangeRate) {
    return (<tr key={exchangeRate.fromCurrency} className={styles.tableRow}>
      <td className={styles.text}>{exchangeRate.fromCurrency}</td>
      <td className={styles.number}>{this.renderCurrencyINR(exchangeRate.baseConversionRate)}</td>
      <td className={styles.number}>{exchangeRate.currencyFluctuationCoefficient}</td>
      <td className={styles.number}>{this.renderCurrencyINR(exchangeRate.definedConversionRate)}</td>
      {this.renderDate(exchangeRate.appliedFrom)}
    </tr>);
  }

  renderDate(date) {
    const FormattedDate = new Date(date);
    return <td className={styles.number}>
      {FormattedDate.getDate()}/{(FormattedDate.getMonth() + 1).toString()}/{FormattedDate.getFullYear()}
    </td>
  }

  renderCurrencyINR(value) {
    return value ? <span className={styles.currencyContainer}>
                <span className={styles.moneyValue}>{value}</span>
                <span className={styles.moneyCurrency}>INR</span>
            </span> : '-';
  }

  render() {
    let exchangeRateElement = null;
    if (this.props.exchangeRates) {
      if (this.props.exchangeRates.length > 0) {
        exchangeRateElement = (<table className={styles.table}>
          {this.renderHeader()}
          <tbody>
          {this.props.exchangeRates.map(exchangeRate => this.renderExchangeRate(exchangeRate))}
          </tbody>
        </table>);
      } else {
        exchangeRateElement = (<h3 className={styles.errorMessage}>{'No current exchange rates are found.'}</h3>);
      }
    }
    else if (this.props.exchangeRatesError !== undefined && this.props.exchangeRatesError !== '') {
      exchangeRateElement = (<h3 className={styles.errorMessage}>{this.props.exchangeRatesError}</h3>);
    }
    else {
      exchangeRateElement = <Loading />;
    }
    return (<div className={styles.exchangeRate}>
      <header className={styles.header}>
        <h2 className={styles.title}>Exchange Rates</h2>
        <PermissionedForNonComponents allowedPermissions={getViewExchangeRateHistoryPermissions()}>
        <div className={styles.goToHistoryContainer}>
          <Link className={styles.goToHistory} to="/exchange-rates-history">View Exchange Rate History</Link>
        </div>
        </PermissionedForNonComponents>
      </header>
      {exchangeRateElement}
    </div>);
  }
}
